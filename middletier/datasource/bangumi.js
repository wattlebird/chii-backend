const { RESTDataSource } = require('apollo-datasource-rest')

const web_url = 'https://api.bgm.tv'

class bangumiAPI extends RESTDataSource {
  constructor() {
    // global client options
    super()
    this.baseURL = web_url;
  }

  async getSubject(id) {
    return this.get(`${this.baseURL}/subject/${id}`);
  }

}

module.exports = bangumiAPI