namespace FractalsChat.Automaton.Common.Extensions
{
    public static class StreamWriterExtension
    {
        public async static Task SendAsync(this StreamWriter writer, string command)
        {
            await writer.WriteLineAsync(command);
            await writer.FlushAsync();
        }

        public async static Task SendAsync(this StreamWriter writer, string to, string body)
        {
            await writer.WriteLineAsync($"PRIVMSG {to} :{body}");
            await writer.FlushAsync();
        }

        public async static Task SendAsync(this StreamWriter writer, string to, string[] body)
        {
            foreach (string line in body)
                await writer.WriteLineAsync($"PRIVMSG {to} :{line}");

            await writer.FlushAsync();
        }
    }
}
