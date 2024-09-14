using CQRS.Core.Command;

namespace Post.Cmd.Api.Commands
{
    public class NewPostCommand : BaseCommand
    {
        public string Author{get;set;}

        public string Message{get;set;}
    }
}