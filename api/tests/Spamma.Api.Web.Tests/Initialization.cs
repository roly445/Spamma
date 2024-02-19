using System.Runtime.CompilerServices;
using DiffEngine;

namespace Spamma.Api.Web.Tests
{
    public static class Initialization
    {
        [ModuleInitializer]
        public static void Run()
        {
            DiffRunner.Disabled = true;
        }
    }
}