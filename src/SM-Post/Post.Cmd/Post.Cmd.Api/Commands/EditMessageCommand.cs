using CQRS.Core.Command;

namespace Post.Cmd.Api.Commands
{
    public class EditMessageCommand : BaseCommand
    {
        public string Message{get;set;}   
    }
}