using System;
using System.Collections.Generic;
using System.Net.Http;
using RuriLib.Attributes;
using RuriLib.Logging;
using RuriLib.Models.Bots;
using Newtonsoft.Json;

namespace Blazor_by_SirTenzin.Blocks.Blaze
{


    [BlockCategory("Blaze", "Blazor's main blocks", "#fff", "#000")]
    public class Blaze
    {
        public class Response
        {
            public bool ran { get; set; }
            public string output { get; set; }
            public string stdout { get; set; }
            public string stderr { get; set; }
            public string version { get; set; }
        }

        private static readonly HttpClient client = new HttpClient();

        string[] Language =
        {
            "awk",
            "bash",
            "brainfuck",
            "c",
            "cpp",
            "crystal",
            "csharp",
            "d",
            "dash",
            "deno",
            "elixir",
            "emacs",
            "elisp",
            "go",
            "haskell",
            "java",
            "jelly",
            "julia",
            "kotlin",
            "lisp",
            "lua",
            "nasm",
            "nasm64",
            "nim",
            "node",
            "osabie",
            "paradoc",
            "perl",
            "php",
            "python2",
            "python3",
            "ruby",
            "rust",
            "scala",
            "swift",
            "typescript",
            "zig",
        };


        [Block("Executes code using the piston execution engine", name = "Piston (P-API)")]
        public string Piston(BotData data, string lang, string code, string stdin)
        {
            bool v = Array.Exists(Language, l => l == lang.ToLower());

            if (v)
            {

                var values = new Dictionary<string, string>
                {
                    { "language", lang },
                    { "source", code },
                    { "stdin", stdin }
                };

                var content = new FormUrlEncodedContent(values);

                var response = client.PostAsync("https://emkc.org/api/v1/piston/execute", content);

                var responseString = response.
                    ConfigureAwait(true)
                    .GetAwaiter()
                    .GetResult()
                    .Content.ReadAsStringAsync()
                    .ConfigureAwait(true)
                    .GetAwaiter()
                    .GetResult();
                if(responseString.Contains("message"))
                {
                    data.Logger.LogHeader();
                    data.Logger.Log($"Error: {responseString.Split("")}");
                    return "Failed";
                }

                Response json = JsonConvert.DeserializeObject<Response>(responseString);
                string stdout = json.stdout;
                string stderr = json.stderr;

                data.Logger.LogHeader();
                data.Logger.Log($"Executed code with STDOut: {stdout}\nSTDErr: {stderr}", LogColors.YellowGreen);
                return stdout;
            } else
            {
                data.Logger.LogHeader();
                data.Logger.Log($"Error: Please use a support langauge: {Language.ToString()}");
                return "Failed";
            }
        }
    }
}
