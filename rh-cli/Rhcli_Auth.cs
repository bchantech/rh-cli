// The MIT License (MIT)
// 
// Copyright (c) 2016 Brendan Chan
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BasicallyMe.RobinhoodNet;

namespace rh_cli
{
    partial class Program
    {
        protected static string symbol;

        static readonly string __tokenFile = System.IO.Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "RobinhoodNet",
        "token");

        static bool Auth(RobinhoodClient client)
        {

            if (System.IO.File.Exists(__tokenFile))
            {
                var token = System.IO.File.ReadAllText(__tokenFile);
                if (!client.Authenticate(token))
                {
                    if (System.IO.File.Exists(__tokenFile))
                    {
                        System.IO.File.Delete(__tokenFile);
                    }
                    return false;
                }
                return true;
            }
            else
            {
                Console.Write("username: ");
                string userName = Console.ReadLine();

                Console.Write("password: ");
                string password = Console.ReadLine();

                if (!client.Authenticate(userName, password))
                {
                    return false;
                }

                System.IO.Directory.CreateDirectory(
                    System.IO.Path.GetDirectoryName(__tokenFile));

                System.IO.File.WriteAllText(__tokenFile, client.AuthToken);
                return true;
            }
        }
    }
}

