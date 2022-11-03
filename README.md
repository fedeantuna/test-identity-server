# Test Identity Server

This project is meant only for testing purposes, DO NOT USE IN A PRODUCTION SCENARIO.

Calling the /connect/token endpoint with

- client_id = test-client
- client_secret = test-client-secret
- scope = test-scope
- grant_type = client_credentials

will return a JSON with the following structure

```
{
    "access_token": "access_token",
    "token_type": "token_type",
    "expires_in": 3600
}
```

The request must be made using the Form URL Encode structure:

```
curl --request POST \
  --url https://localhost:7018/connect/token \
  --header 'Content-Type: application/x-www-form-urlencoded' \
  --data client_id=test-client \
  --data client_secret=test-client-secret \
  --data scope=test-scope \
  --data grant_type=client_credentials
```
