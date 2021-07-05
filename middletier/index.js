const { ApolloServer, gql } = require('apollo-server');
const typeDef = require("./graphql/chii");
const resolvers = require("./resolver/chii_resolver");
const chiiAPI = require("./datasource/chii")


const server = new ApolloServer({
  typeDefs: typeDef,
  resolvers,
  dataSources: () => {
    return {
      chiiAPI: new chiiAPI()
    }
  }
});

server.listen().then(({ url }) => {
  console.log(`🚀  Server ready at ${url}`);
});