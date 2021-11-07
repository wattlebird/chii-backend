const { RESTDataSource } = require('apollo-datasource-rest')

const web_url = process.env.WEB_URL || 'http://localhost:5000/'

class chiiAPI extends RESTDataSource {
  constructor() {
    // global client options
    super()
    this.baseURL = web_url;
  }

  async getLastDate() {
    return this.get(`/api/misc/lastdate`)
  }

  async getSubjectCount(type, ranked) {
    return this.get(`/api/subjects/count`, {
      type,
      ranked
    })
  }

  async getRankedList(type, from, step, bysci) {
    return this.get(`/api/subjects/ranked`,  {
      type,
      from,
      step,
      bysci
    })
  }

  async getTags() {
    return this.get(`/api/tags`)
  }

  async searchSubjectByTags(tags, minVoters, minFavs) {
    return this.post(`/api/tags/search`,  {
      tags,
      minVoters,
      minFavs
    })
  }

  async searchRelatedTags(tags) {
    return this.post(`/api/tags/related`, {
      tags
    })
  }

  async getSubject(id) {
    return this.get(`/api/subjects/${id}`)
  }
}

module.exports = chiiAPI