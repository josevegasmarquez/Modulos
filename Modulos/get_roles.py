import requests
import urllib3

urllib3.disable_warnings()
base_url = "https://localhost:5207/api"

def get_roles():
    login_url = f"{base_url}/Auth/login"
    login_payload = {"email": "superadmin@modulos.com", "password": "Jlvm2612@"}
    login_response = requests.post(login_url, json=login_payload, verify=False)
    token = login_response.json().get('token')
    
    roles_url = f"{base_url}/Auth/roles"
    headers = {"Authorization": f"Bearer {token}"}
    roles_response = requests.get(roles_url, headers=headers, verify=False)
    print(roles_response.text)

if __name__ == "__main__":
    get_roles()
