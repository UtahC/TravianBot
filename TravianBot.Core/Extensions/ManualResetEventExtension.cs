using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TravianBot.Core.Extensions
{
    static class ManualResetEventExtension
    {
        public static async Task AwaitOne(this ManualResetEvent mre)
        {
            await new Task(() => mre.WaitOne());
        }
    }
}
