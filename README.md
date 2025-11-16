# 🚀  NewsFetcher API — HTTP & HTTPS Setup Guide

This project supports **both HTTP and HTTPS** endpoints.

## 🧭 Quick Start (Http Profile)

```bash
git clone https://github.com/irfankhan55/NewsFetcherAPI.git
cd NewsFetcherAPI
cd src\NewsFetcher.Api
dotnet build
dotnet run
```

Open Swagger UI in your browser: [http://localhost:5000/swagger/index.html](http://localhost:5000/swagger/index.html)

[![Swagger UI](/docs/images/SwaggerUI.png)]

try any endpoint by clicking Execute Button

## 🧭 Running Https Profile

### step 1.
uncomment the following lines from 'appsettings.json' file.

[![https comments](/docs/images/https-comments.png)]

### step 2.
You need a self signed certificate to run 'HTTPS' profile. In the Base folder 'NewsFetcherAPI' please run the following commands one by one.

```bash
mkdir certs

openssl genrsa -out certs/devcert.key 2048

openssl req -x509 -new -nodes -key certs/devcert.key -sha256 -days 365 \
-out certs/devcert.crt -subj "/CN=localhost"

openssl pkcs12 -export -out certs/devcert.pfx -inkey certs/devcert.key -in certs/devcert.crt -password pass:1234

sudo security add-trusted-cert -d -r trustRoot -k /Library/Keychains/System.keychain certs/devcert.crt

```

Enter your password if prompted.

You will now see three files under the certs folder. 
 
 [![Certs](/docs/images/certs.png)]

### step 3.
Goto your keychanin and make sure the localhost certificate in your keychain is always trusted.

### step 4.

```bash
dotnet clean

dotnet build

dotnet run --launch-profile "https"
```
### step 5.

Open Swagger UI in your browser with https profile: [https://localhost:7184/swagger/index.html](https://localhost:7184/swagger/index.html)