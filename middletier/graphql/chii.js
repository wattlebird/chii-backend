const { gql } = require('apollo-server')

const typeDef = gql`
  enum SubjectType {
    ANIME
    BOOK
    GAME
    MUSIC
    REAL
  }

  type Tag {
    tag: String!,
    tagCount: Int!,
    userCount: Int!
    confidence: Float!
  }

  type BriefTag {
    tag: String!,
    coverage: Int!,
    confidence: Float!
  }

  type Subject {
    id: ID!,
    name: String!,
    nameCN: String,
    type: SubjectType!,
    rank: Int,
    sciRank: Int,
    date: String,
    votenum: Int!,
    favnum: Int!,
    tags: [Tag]
  }

  type Query {
    queryRankingDate: String,
    queryRankingList(bysci: Boolean, from: Int, step: Int): [Subject],
    queryRankingCount: Int,
    getTagList: [BriefTag],
    searchByTag(tags: [String], minVoters: Int, minFavs: Int): [Subject],
    searchRelatedTags(tags: [String]): [BriefTag]
  }
`

module.exports = typeDef