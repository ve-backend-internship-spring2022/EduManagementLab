// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


#pragma warning disable 1591

namespace EduManagementLab.Core.Entities.client
{
    public class ClientClaim
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
        public OAuthClient Client { get; set; }
    }
}