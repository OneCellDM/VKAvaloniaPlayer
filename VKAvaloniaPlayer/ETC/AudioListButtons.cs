using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Controls;
using ReactiveUI;
using VKAvaloniaPlayer.ETC;
using VKAvaloniaPlayer.Models;

namespace VKAvaloniaPlayer.ViewModels
{
	public class AudioListButtons : ReactiveObject
	{
		public AudioAlbumModel Album { get; set; } = null;

		private bool _AudioDownloadIsVisible;
		private bool _AudioAddIsVisible;
		private bool _AudioRemoveIsVisible;
		private bool _AudioAddToAlbumIsVisible;

		public bool AudioDownloadIsVisible
		{
			get => _AudioDownloadIsVisible;

			set => this.RaiseAndSetIfChanged(ref _AudioDownloadIsVisible, value);
		}

		public bool AudioAddIsVisible
		{
			get => _AudioAddIsVisible;
			set => this.RaiseAndSetIfChanged(ref _AudioAddIsVisible, value);
		}

		public bool AudioRemoveIsVisible
		{
			get => _AudioRemoveIsVisible;
			set => this.RaiseAndSetIfChanged(ref _AudioRemoveIsVisible, value);
		}

		public bool AudioAddToAlbumIsVisible
		{
			get => _AudioAddToAlbumIsVisible;
			set => this.RaiseAndSetIfChanged(ref _AudioAddToAlbumIsVisible, value);
		}

		public IReactiveCommand AudioAddCommand { get; set; }
		public IReactiveCommand AudioDownloadCommand { get; set; }
		public IReactiveCommand AudioRemoveCommand { get; set; }
		public IReactiveCommand AudioAddToAlbumCommand { get; set; }

		public AudioListButtons()
		{
			_AudioAddIsVisible = true;
			_AudioAddToAlbumIsVisible = true;
			_AudioDownloadIsVisible = true;
			_AudioRemoveIsVisible = true;
			AudioAddCommand = ReactiveCommand.Create(async (Avalonia.Interactivity.RoutedEventArgs e) =>
			{
				var button = (e.Source as Button);

				var vkModel = button.DataContext as AudioModel;

				if (vkModel != null)
				{
					var res = await ETC.GlobalVars.VkApi.Audio.AddAsync(vkModel.ID, vkModel.OwnerID, vkModel.AccessKey);
					if (res > 0)
						AllMusicViewModel.AudioAddEventCall(vkModel);
				}
			});
			AudioAddToAlbumCommand = ReactiveCommand.Create((Avalonia.Interactivity.RoutedEventArgs e) =>
			{
			});

			AudioDownloadCommand = ReactiveCommand.Create(async (Avalonia.Interactivity.RoutedEventArgs e) =>
			{
				var button = (e.Source as Button);

				var vkModel = button.DataContext as AudioModel;

				if (vkModel != null)
				{
					if (vkModel.IsDownload)
						return;

					vkModel.IsDownload = true;

					try
					{
						var res = await ETC.GlobalVars.VkApi.Audio.GetByIdAsync(new string[] { vkModel.GetAudioIDFormatWithAccessKey() });

						using (WebClient webClient = new WebClient())
						{
							webClient.DownloadFileAsync(res.ElementAt(0).Url, string.Format("{0}-{1}.mp3", vkModel.Artist, vkModel.Title));
							webClient.DownloadFileCompleted += delegate { vkModel.IsDownload = false; };
							webClient.DownloadProgressChanged += (object o, DownloadProgressChangedEventArgs e) => vkModel.DownloadPercent = e.ProgressPercentage;
						}
					}
					catch
					{
						vkModel.IsDownload = false;
					}
				}
			});

			AudioRemoveCommand = ReactiveCommand.Create(async (Avalonia.Interactivity.RoutedEventArgs e) =>
			{
				var button = (e.Source as Button);

				var vkModel = button.DataContext as AudioModel;

				if (vkModel != null)
				{
					if (Album is null)
					{
						var Awaiter = await ETC.GlobalVars.VkApi.Audio.DeleteAsync(vkModel.ID, vkModel.OwnerID);
						try
						{
							var taskAwaiter2 = await ETC.GlobalVars.VkApi.Audio.GetByIdAsync(
								new string[] { vkModel.GetAudioIDFormatWithAccessKey() });
						}
						catch (VkNet.Exception.ParameterMissingOrInvalidException ex)
						{
							AllMusicViewModel.AudioRemoveEventCall(vkModel);
						}
					}
					else
					{
						List<string> audios = new List<string>();
						try
						{
							var Audiosres = await ETC.GlobalVars.VkApi.Audio.GetAsync(new VkNet.Model.RequestParams.AudioGetParams()
							{
								OwnerId = Album.OwnerID,
								PlaylistId = Album.ID,
								Count = 6000,
							});
							for (int i = 0; i < Audiosres.Count; i++)
							{
								if (Audiosres[i].Id == vkModel.ID)
									continue;

								audios.Add(vkModel.GetAudioIDFormatNoAccessKey());
							}

							var res = ETC.GlobalVars.VkApi.Audio.EditPlaylist(Album.OwnerID, (int)Album.ID, Album.Title, null, audios);

							if (res)
								MusicFromAlbumViewModel.AudioRemoveEventCall(vkModel);
						}
						catch (Exception ex) { }
						finally
						{
							audios.Clear();
						}
					}
				}
			});
		}
	}
}