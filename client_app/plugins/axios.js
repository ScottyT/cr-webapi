export default function ({ $axios, error: nuxtError, store, redirect, app }, inject) {
  const api = $axios.create({
    headers: {
      common: {
        Accept: 'application/json'
      }
    }
  })
  const gcs = $axios.create({
    headers: {
      common: {
        Accept: 'application/json'
      }
    }
  })
  api.onRequest(() => {
    try {
      const token = app.$auth.strategy.token.get().split(' ')[1];
      api.setToken(token, 'Bearer')
    } catch (error) {
      console.log("Could not set token:", error)
    }
  })
  gcs.onRequest(() => {
    try {
      const token = app.$auth.strategy.token.get().split(' ')[1];
      gcs.setToken(token, 'Bearer')
    } catch (error) {
      console.log("Could not set token:", error)
    }
  })
  var baseurl = process.env.NODE_ENV !== 'production' ? 'https://localhost:7255' : process.env.apiUrl
  api.setBaseURL(baseurl);
  inject('api', api);

  var gcsUrl = process.env.NODE_ENV !== 'production' ? 'http://localhost:8081' : process.env.gsutil
  gcs.setBaseURL(gcsUrl);
  inject('gcs', gcs);
}