import requests
import json
import urllib3
import random

urllib3.disable_warnings(urllib3.exceptions.InsecureRequestWarning)

base_url = "https://localhost:5207/api"

def login():
    url = f"{base_url}/Auth/login"
    payload = {"email": "superadmin@modulos.com", "password": "Jlvm2612@"}
    response = requests.post(url, json=payload, verify=False)
    return response.json().get('token')

def test_register(token, role):
    url = f"{base_url}/Auth/register"
    headers = {"Authorization": f"Bearer {token}"}
    dni = str(random.randint(10000000, 99999999))
    email = f"test_{role.lower()}_{dni}@example.com"
    payload = {
        "nombres": "Test",
        "apellidos": role,
        "dni": dni,
        "email": email,
        "password": "Password123!",
        "fechaNacimiento": "1990-01-01",
        "genero": 0,
        "role": role
    }
    response = requests.post(url, json=payload, headers=headers, verify=False)
    print(f"Testing Role '{role}': Status {response.status_code}")
    if response.status_code == 200:
        res_json = response.json()
        assigned_roles = res_json['user']['roles']
        print(f"  Assigned Roles: {assigned_roles}")
        if role in assigned_roles:
            print(f"  SUCCESS: Role '{role}' correctly assigned.")
        else:
            print(f"  FAILURE: Role '{role}' NOT found in assigned roles.")
    else:
        print(f"  ERROR: {response.text}")

if __name__ == "__main__":
    token = login()
    if token:
        roles_to_test = ["Admin", "Usuario", "Almacenero", "InvalidRole"]
        for role in roles_to_test:
            test_register(token, role)
