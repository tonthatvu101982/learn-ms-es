using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Command;

namespace Post.Cmd.Api.Commands
{
    public class DeletePostCommand : BaseCommand
    {
        public string Username{get;set;}
    }
}