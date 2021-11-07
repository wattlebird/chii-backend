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
    querySubject: async (_, { id }, { dataSources }) => {
      return dataSources.chiiAPI.getSubject(id);
    },
    getTagList: async(_, __, { dataSources }) => {
      return dataSources.chiiAPI.getTags();
    },
    searchByTag: async(_, {tags, minVoters, minFavs}, {dataSources}) => {
      return dataSources.chiiAPI.searchSubjectByTags(tags, minVoters, minFavs);
    },
    searchRelatedTags: async(_, {tags}, {dataSources}) => {
      return dataSources.chiiAPI.searchRelatedTags(tags);
    },

    queryBangumiSubject: (_, { id }, { dataSources }) => {
      return dataSources.bangumiAPI.getSubject(id)
    },
  }
}

module.exports = resolvers