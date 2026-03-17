import requests
import json
import urllib3

urllib3.disable_warnings(urllib3.exceptions.InsecureRequestWarning)

base_url = "https://localhost:5207/api"

def login():
    url = f"{base_url}/Auth/login"
    payload = {
        "email": "superadmin@modulos.com",
        "password": "Jlvm2612@"
    }
    response = requests.post(url, json=payload, verify=False)
    if response.status_code == 200:
        data = response.json()
        print(f"Login Success. Token length: {len(data['token'])}")
        return data['token']
    else:
        print(f"Login Failed: {response.status_code}")
        print(response.text)
        return None

def test_register(token, role, email_suffix):
    url = f"{base_url}/Auth/register"
    headers = {
        "Authorization": f"Bearer {token}",
        "Content-Type": "application/json"
    }
    payload = {
        "nombres": f"Test {role}",
        "apellidos": "User",
        "dni": f"9999{email_suffix}", # unique-ish DNI
        "email": f"test{email_suffix}@{role.lower()}.com",
        "password": "Password123!",
        "fechaNacimiento": "1990-01-01",
        "genero": 0,
        "direccion": "Calle Falsa 123",
        "phoneNumber": "987654321",
        "role": role
    }
    response = requests.post(url, json=payload, headers=headers, verify=False)
    print(f"Register Role '{role}' Response: {response.status_code}")
    print(response.text)

if __name__ == "__main__":
    token = login()
    if token:
        test_register(token, "Admin", "101")
        test_register(token, "User", "102")
        test_register(token, "SuperAdmin", "103")
