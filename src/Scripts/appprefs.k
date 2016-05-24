co = Connect()

function getprefs(){
	return co.GetManager("ApplicationPreferences")
}

prefs = getprefs()
timeout = prefs.WsTimeoutInMs
Write(prefs.WsTimeoutInMs)
prefs.WsTimeoutInMs = 300
Write(prefs.WsTimeoutInMs)

prefs.WsTimeoutInMs = timeout
