# Distributed Network Applications II
## TalTech magister course ITV0120/ITI0215

### Andres Käver (183263IAPM) & Anu Kuusmaa (182912IAPM)


Implementation of simple distributed ledger as ASP.NET Core middleware.

Initial spec for work is/was published here: 
http://lambda.ee/wiki/Vorgurakendused_2_prax_1_2019_kevad (in estonian).  
Also saved as pdf (spec01.pdf) in solution root folder for historical purposes.

Second stage spec is here: 
http://lambda.ee/wiki/Vorgurakendused_2_prax_1_2019_kevad  
Also saved as pdf (spec02.pdf) in solution root folder for historical purposes.  


Project is/was developed using JetBrains Rider. Dev environment is based on macOS Mojave.  


Misc endpoints  
/ledger/log  
Get the log entries from this host  


Endpoints for p2p discovery  
/ledger/addr/?addr=<clientip>&port=<clientport>  
parameters: addr=127.0.0.1&port=5001 - incoming host also includes its own public ip/port  
response: list of hosts known currently - new hosts get added to db  
 
/ledger/ping  
Used during /ledger/addr request to check, that incoming host public info is correct  
response: json object with hosts public key.  


Endpoints for distributed ledger  
/ledger/blocks?from=<hash>&to=<hash>  
get known blocks in chain from host. from - null for genesis block, to - null for till the end  

/ledger/createblock?content=<somecontent>  
insert new block at the end of the chain - to be synced into ledger  
  
/ledger/singleblock?hash=<blockhash>  
/ledger/singleblock?hash=<contenthash>  
Get block either by content or block hash  

POST  
/ledger/receiveblock/?addr=<clientip>&port=<clientport>&hash=<block_content_hash>  
Actual block content as json in post body  


