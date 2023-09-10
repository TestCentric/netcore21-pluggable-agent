// ***********************************************************************
// Copyright (c) Charlie Poole and TestCentric contributors.
// Licensed under the MIT License. See LICENSE file in root directory.
// ***********************************************************************

using System;
using System.Diagnostics;
using System.IO;
using System.Security;
using TestCentric.Engine.Internal;
using TestCentric.Engine.Communication.Transports.Tcp;
using TestCentric.Agents;

namespace TestCentric.Engine.Agents
{
    public class NetCore21Agent : TestCentricAgent<NetCore21Agent>
    {
        public static void Main(string[] args) => TestCentricAgent<NetCore21Agent>.Execute(args);
    }
}
