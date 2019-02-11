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
osascript <<EOF
tell application "iTerm"
    tell current window
	create tab with default profile
	tell current session
	    write text "cd \"`pwd`\";cd ..;cd WebApp;dotnet run 5001 /Users/akaver/Magister/VR2/P2P/Data/5001.json;exit"
	end tell
    end tell
end tell
EOF
osascript <<EOF
tell application "iTerm"
    tell current window
	create tab with default profile
	tell current session
	    write text "cd \"`pwd`\";cd ..;cd WebApp;dotnet run 5002 /Users/akaver/Magister/VR2/P2P/Data/5002.json;exit"
	end tell
    end tell
end tell
EOF
osascript <<EOF
tell application "iTerm"
    tell current window
	create tab with default profile
	tell current session
	    write text "cd \"`pwd`\";cd ..;cd WebApp;dotnet run 5003 /Users/akaver/Magister/VR2/P2P/Data/5003.json;exit"
	end tell
    end tell
end tell
EOF
osascript <<EOF
tell application "iTerm"
    tell current window
	create tab with default profile
	tell current session
	    write text "cd \"`pwd`\";cd ..;cd WebApp;dotnet run 5004 /Users/akaver/Magister/VR2/P2P/Data/5004.json;exit"
	end tell
    end tell
end tell
EOF
osascript <<EOF
tell application "iTerm"
    tell current window
	create tab with default profile
	tell current session
	    write text "cd \"`pwd`\";cd ..;cd WebApp;dotnet run 5005 /Users/akaver/Magister/VR2/P2P/Data/5005.json;exit"
	end tell
    end tell
end tell
EOF
osascript <<EOF
tell application "iTerm"
    tell current window
	create tab with default profile
	tell current session
	    write text "cd \"`pwd`\";cd ..;cd WebApp;dotnet run 5006 /Users/akaver/Magister/VR2/P2P/Data/5006.json;exit"
	end tell
    end tell
end tell
EOF
osascript <<EOF
tell application "iTerm"
    tell current window
	create tab with default profile
	tell current session
	    write text "cd \"`pwd`\";cd ..;cd WebApp;dotnet run 5007 /Users/akaver/Magister/VR2/P2P/Data/5007.json;exit"
	end tell
    end tell
end tell
EOF
osascript <<EOF
tell application "iTerm"
    tell current window
	create tab with default profile
	tell current session
	    write text "cd \"`pwd`\";cd ..;cd WebApp;dotnet run 5008 /Users/akaver/Magister/VR2/P2P/Data/5008.json;exit"
	end tell
    end tell
end tell
EOF
osascript <<EOF
tell application "iTerm"
    tell current window
	create tab with default profile
	tell current session
	    write text "cd \"`pwd`\";cd ..;cd WebApp;dotnet run 5009 /Users/akaver/Magister/VR2/P2P/Data/5009.json;exit"
	end tell
    end tell
end tell
EOF
