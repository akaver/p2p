#!/bin/sh

osascript <<EOF
tell application "iTerm"
    tell current window
	create tab with default profile
	tell current session
	    write text "cd \"`pwd`\";$1;exit"
	end tell
    end tell
end tell
EOF
