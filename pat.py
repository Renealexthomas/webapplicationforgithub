import requests

url = "https://vssps.dev.azure.com/vstsustglobal/_apis/graph/users?subjectKind=user&api-version=7.1-preview.1"
headers = {
    "Authorization": "Basic <base64-encoded-PAT>",
    "Content-Type": "application/json"
}
data = {
    "subjectKind": "user",
    "email": "183597@ust.com"  # Replace with the user's email
}

response = requests.post(url, headers=headers, json=data)

print(response.status_code)
print(response.json())