import requests
import base64

# Replace placeholders
organization = "vstsustglobal"
project = "USTC-ISXX-DG-WE_REFM-DigiSecure"
pat = "37Se7XVPraDGp9F4cc2m1CZHnpqESSIW2McdX4wSlN8s9zjFP6bZJQQJ99BAACAAAAAhG3NWAAASAZDO3yU1"
user_email = "183597@ust.com"

# Encode PAT for Basic Auth
credentials = base64.b64encode(f":{pat}".encode()).decode()

# API endpoint
url = f"https://dev.azure.com/{organization}/{project}/_apis/wit/wiql?api-version=7.1-preview.2"

# Headers
headers = {
    "Authorization": f"Basic {credentials}",
    "Content-Type": "application/json"
}

# WIQL query body
body = {
    "query": f"SELECT [System.Id], [System.Title], [System.State] FROM WorkItems WHERE [System.AssignedTo] = '{user_email}'"
}

# Send POST request
response = requests.post(url, headers=headers, json=body)

if response.status_code == 200:
    work_items = response.json().get("workItems", [])
    print(f"Found {len(work_items)} work items.")
    for item in work_items:
        print(f"ID: {item['id']}, Title: {item['fields']['System.Title']}, Status: {item['fields']['System.State']}")
else:
    print(f"Error: {response.status_code} - {response.text}")