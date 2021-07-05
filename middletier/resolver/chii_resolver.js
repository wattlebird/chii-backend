const resolvers = {
  SubjectType: {
    ANIME: "anime",
    BOOK: "book",
    GAME: "game",
    MUSIC: "music",
    REAL: "real"
  },

  Query: {
    queryRankingDate: async (_, __, { dataSources }) => {
      return dataSources.chiiAPI.getLastDate();
    },
    queryRankingCount: async (_, __, { dataSources }) => {
      return dataSources.chiiAPI.getSubjectCount("anime", true);
    },
    queryRankingList: async (_, {bysci, from, step}, { dataSources }) => {
      return dataSources.chiiAPI.getRankedList('anime', from, step, bysci);
    },
    getTagList: async(_, __, { dataSources }) => {
      return dataSources.chiiAPI.getTags("anime");
    },
    searchByTag: async(_, {tags, minVoters, minFavs}, {dataSources}) => {
      return dataSources.chiiAPI.searchSubjectByTags("anime", tags, minVoters, minFavs);
    },
    searchRelatedTags: async(_, {tags}, {dataSources}) => {
      return dataSources.chiiAPI.searchRelatedTags("anime", tags);
    }
  }
}

module.exports = resolvers