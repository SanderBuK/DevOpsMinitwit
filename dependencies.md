Go to http://www.webgraphviz.com/ and paste the following:

```
digraph application {

  "Kibana" -> "Elasticsearch"
  "Kibana" -> "Nginx"
  "Elasticsearch" -> "API"
  "Elasticsearch" -> "Nginx"
   "API" -> "Serilog"

  "Grafana" -> "Prometheus"
  "Prometheus" -> "API"
  "API" -> "Prometheus-net"

  "Blazor" -> "API"
  "Blazor" -> "SessionStorage"
  "Blazor" -> "EntityFramework"
  "Blazor" -> "MySQL"

  "API" -> "Microsoft CachingMemory"
  "API" -> "ASP.Net Core"
  "API" -> "EntityFramework"
  "API" -> "MySQL"

  "Docker-compose" -> "Grafana"
  "Docker-compose" -> "Kibana"
  "Docker-compose" -> "Blazor"
  "Docker-compose" -> "API"
  "Docker-compose" -> "Prometheus"
  "Docker-compose" -> "Elasticsearch"

  "Github Actions" -> "Docker-compose"
}
```
