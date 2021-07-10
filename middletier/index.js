const { ApolloServer } = require('apollo-server');
const { BaseRedisCache } = require('apollo-server-cache-redis');
const Redis = require('ioredis');
const typeDef = require("./graphql/chii.graphql");
const resolvers = require("./resolver/chii_resolver");
const chiiAPI = require("./datasource/chii")

const redis_host = process.env.REDIS_HOST || "localhost"

const server = new ApolloServer({
  typeDefs: typeDef,
  resolvers,
  cache: new BaseRedisCache({
    client: new Redis({
      port: 6379,
      host: redis_host,
    }),
  }),
  dataSources: () => {
    return {
      chiiAPI: new chiiAPI()
    }
  }
});

server.listen({
  port: 4000,
  host: "0.0.0.0"
}).then(({ url }) => {
  console.log(`🚀  Server ready at ${url}`);
});