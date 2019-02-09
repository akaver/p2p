#!/bin/sh

osascript <<EOF
tell application "iTerm"
    tell current window
	create tab with default profile
	tell current session
	    write text "cd \"`pwd`\";cd ..;cd WebApp;dotnet run 5000 /Users/akaver/Magister/VR2/P2P/Data/5000.json;exit"
	end tell
    end tell
end tell
EOF

