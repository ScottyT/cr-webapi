export const actions = {
  async nuxtServerInit({ state, dispatch, commit }, { res, $axios, $auth }) {
    /* if ($auth.loggedIn) {
      const { allClaims: claims, idToken: token, ...authUser } = $auth.user
      console.log("auth user from server init: ", authUser)
      await dispatch("users/fetchUser", authUser)
      return;
    } */
    //commit('users/setLoading', true)
    //Continue to test this later
    /* console.log(res.locals)
    if (res && res.locals && res.locals.user) {
      const { allClaims: claims, idToken: token, ...authUser } = res.locals.user
      console.log("user: ", res.locals.user)
    } */
    /* commit('users/setUser', {
      email: null,
      id: null,
      role: null,
      name: null,
      certifications: null
    }) */
  },
}
