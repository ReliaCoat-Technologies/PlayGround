﻿// See https://aka.ms/new-console-template for more information

using LinuxDockerContainerExecute;

var a = new BashScriptRunner("/usr/bin/whoami");
a.executeAsync();

Console.ReadKey();
