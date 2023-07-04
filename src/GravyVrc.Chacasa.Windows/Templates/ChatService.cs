using BuildSoft.VRChat.Osc.Chatbox;

namespace GravyVrc.Chacasa.Windows.Templates;

public static class ChatService
{
    public delegate void MessageSentEventHandler(string message);

    public static event MessageSentEventHandler? MessageSent;

    public static void SendMessage(string message)
    {
        OscChatbox.SendMessage(message, direct: true);
        MessageSent?.Invoke(message);
    }
}