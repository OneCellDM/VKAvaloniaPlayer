using System;
using System.Linq;
using Avalonia.Input;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using VKAvaloniaPlayer.ETC;
using VKAvaloniaPlayer.Models;
using VKAvaloniaPlayer.Notify;
using VKAvaloniaPlayer.ViewModels.Interfaces;

namespace VKAvaloniaPlayer.ViewModels.Audios;

public class AddToAlbumViewModel : 
    AlbumsViewModel,ICloseView
{
    private AudioModel _AudioModel;
    public AddToAlbumViewModel(AudioModel audioModel)
    {
        if (audioModel is null)
        {
            Notify.NotifyManager.Instance.PopMessage(
                new NotifyData("Ошибка добавления",$"Аудиозапись не выбрана"));
            throw new ArgumentNullException(nameof(audioModel));
        }
        else
        {
            this._AudioModel = audioModel;
            CloseCommand = ReactiveCommand.Create(() => CloseViewEvent?.Invoke());
        }
    }
    public override void SelectedItem(object sender, PointerPressedEventArgs args)
    {
        var item = args?.GetContent<AudioAlbumModel>();
        if (item != null)
        {
            try
            {
                var ids = new[] { _AudioModel.GetAudioIDFormatWithAccessKey() };
              
                GlobalVars.VkApi.Audio.AddToPlaylist(item.OwnerID, item.ID, ids);
                
                Notify.NotifyManager.Instance.PopMessage(
                    new NotifyData("Успешно добавлено",$"Аудиозапись {_AudioModel.Title} добавлена в альбом {item.Title}"));
            }
            catch (Exception ex)
            {
                Notify.NotifyManager.Instance.PopMessage(
                    new NotifyData("Ошибка добавления",$"Аудиозапись {_AudioModel.Title} не добавлена в альбом {item.Title}"));
            }
        }
        CloseViewEvent?.Invoke();
    }
    protected override void LoadData()
    {
        if (GlobalVars.CurrentAccount?.UserID != null)
        {
            var res = 
                GlobalVars.VkApi.Audio.GetPlaylists((long)GlobalVars.CurrentAccount.UserID, 200,
                (uint)Offset);
            if (res != null)
            {
                DataCollection.AddRange(res.Where(x=>x.Original == null));
                DataCollection.StartLoadImagesAsync();
            }
        }
    }

    public event ICloseView.CloseViewDelegate? CloseViewEvent;
    public IReactiveCommand CloseCommand { get; set; }
}