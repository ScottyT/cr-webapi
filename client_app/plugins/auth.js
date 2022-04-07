export default function({$auth}) {
    $auth.onError((error, name, endpoint) => {
        console.error(name, error)
    })
    $auth.onRedirect((config) => {
        console.log(config)
        //alert("You were logged out for inactivity")
    })
    //console.log("refresh token:", $auth.strategy.refreshToken.get())
    if (!$auth.loggedIn) {
        return
    }
}