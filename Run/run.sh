#!/bin/sh
cd ../WebApp
dotnet build -c Release
cd ../Run

osascript <<EOF
tell application "iTerm"
    tell current window
	create tab with default profile
	tell current session
	    write text "cd \"`pwd`\";cd ..;cd WebApp;dotnet run 5000 ../Data/5000.json;exit"
	end tell
    end tell
end tell
EOF
osascript <<EOF
tell application "iTerm"
    tell current window
	create tab with default profile
	tell current session
	    write text "cd \"`pwd`\";cd ..;cd WebApp;dotnet run 5001 ../Data/5001.json;exit"
	end tell
    end tell
end tell
EOF
osascript <<EOF
tell application "iTerm"
    tell current window
	create tab with default profile
	tell current session
	    write text "cd \"`pwd`\";cd ..;cd WebApp;dotnet run 5002 ../Data/5002.json;exit"
	end tell
    end tell
end tell
EOF
osascript <<EOF
tell application "iTerm"
    tell current window
	create tab with default profile
	tell current session
	    write text "cd \"`pwd`\";cd ..;cd WebApp;dotnet run 5003 ../Data/5003.json;exit"
	end tell
    end tell
end tell
EOF
osascript <<EOF
tell application "iTerm"
    tell current window
	create tab with default profile
	tell current session
	    write text "cd \"`pwd`\";cd ..;cd WebApp;dotnet run 5004 ../Data/5004.json;exit"
	end tell
    end tell
end tell
EOF
osascript <<EOF
tell application "iTerm"
    tell current window
	create tab with default profile
	tell current session
	    write text "cd \"`pwd`\";cd ..;cd WebApp;dotnet run 5005 ../Data/5005.json;exit"
	end tell
    end tell
end tell
EOF
osascript <<EOF
tell application "iTerm"
    tell current window
	create tab with default profile
	tell current session
	    write text "cd \"`pwd`\";cd ..;cd WebApp;dotnet run 5006 ../Data/5006.json;exit"
	end tell
    end tell
end tell
EOF
osascript <<EOF
tell application "iTerm"
    tell current window
	create tab with default profile
	tell current session
	    write text "cd \"`pwd`\";cd ..;cd WebApp;dotnet run 5007 ../Data/5007.json;exit"
	end tell
    end tell
end tell
EOF
osascript <<EOF
tell application "iTerm"
    tell current window
	create tab with default profile
	tell current session
	    write text "cd \"`pwd`\";cd ..;cd WebApp;dotnet run 5008 ../Data/5008.json;exit"
	end tell
    end tell
end tell
EOF
osascript <<EOF
tell application "iTerm"
    tell current window
	create tab with default profile
	tell current session
	    write text "cd \"`pwd`\";cd ..;cd WebApp;dotnet run 5009 ../Data/5009.json;exit"
	end tell
    end tell
end tell
EOF
