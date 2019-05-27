# Distributed Network Applications II
## TalTech magister course ITV0120/ITI0215

### Andres Käver (183263IAPM) & Anu Kuusmaa (182912IAPM)


Implementation of simple distributed ledger as ASP.NET Core middleware.

Initial spec for work is/was published here: 
http://lambda.ee/wiki/Vorgurakendused_2_prax_1_2019_kevad (in estonian).  
Also saved as pdf (spec01.pdf) in solution root folder for historical purposes.

Second stage spec is here: 
http://lambda.ee/wiki/Vorgurakendused_2_prax_2_2019_kevad  
Also saved as pdf (spec02.pdf) in solution root folder for historical purposes.  


Project is/was developed using JetBrains Rider. Dev environment is based on macOS Mojave.  


### API Endpoints

All the endpoints are based on GET unlesss marked with POST (for example /ledger/receiveblock)  

Misc endpoints  
/ledger/log  
Get the log entries from this host  


Endpoints for p2p discovery  
~~~
/ledger/addr/?addr=<client ip>&port=<client port>  
~~~
parameters: addr=127.0.0.1&port=5001 - incoming host also includes its own public ip/port  
response: list of hosts known currently - new hosts get added to db  
 
~~~
/ledger/ping  
~~~
Used during /ledger/addr request to check, that incoming host public info is correct  
response: json object with hosts public key.  


#### Endpoints for distributed ledger  
~~~
/ledger/ledgerhash 
~~~
get ledger hash

~~~
/ledger/blocks?from=<hash>&to=<hash>  
~~~
get known blocks in chain from host. from - null for genesis block, to - null for till the end  

~~~
/ledger/createblock?content=<some content>  
~~~
insert new block at the end of the chain - to be synced into ledger  
  
~~~
/ledger/singleblock?hash=<block hash>  
/ledger/singleblock?payloadhash=<payload hash>  
~~~
Get block either by content or block hash  

POST  
~~~
/ledger/receiveblock/?addr=<client ip>&port=<client port>&hash=<block content hash>  
~~~
Actual block content as json in post body  

### Launching network

Start as many hosts as you like. Command line  
~~~
>dotnet run WebApp.dll <portno> <settings.json>
~~~
portno - port to open for listening
settings - json file, containing keys and initially known hosts.

There is script file in /Run folder - run.sh. Launches 10 hosts in osx iterm - every one in separate tab.

### Monitoring tool

Monitoring tool is written as separate web application, MonitorWeb.  
Allows to monitor p2p/ledger network activity and insert new blocks into ledger.   
