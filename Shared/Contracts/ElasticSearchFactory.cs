﻿using System;

using Nest;

namespace Contracts
{
    public class ElasticSearchFactory
    {
        private static readonly Lazy<ElasticClient> client = new Lazy<ElasticClient>(BuildClient);

        public static ElasticClient GetClient()
        {
            return client.Value;
        }

        private static ElasticClient BuildClient()
        {
            var connection = new ConnectionSettings(new Uri("http://lb-agxv47bobldeo.westeurope.cloudapp.azure.com:9200/"));
            connection.BasicAuthentication("elastic", "Abc1234VUnit");
            connection.DefaultMappingFor<Stiftung>(m => m.IndexName("stiftungen_temp").IdProperty(s => s.id));

            return new ElasticClient(connection);
        }
    }
}
