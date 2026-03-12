
$baseUrl = "https://localhost:5207/api"
$loginUrl = "$baseUrl/Auth/login"
$profileUrl = "$baseUrl/Auth/profile"
$changePasswordUrl = "$baseUrl/Auth/change-password"

# Disable SSL verification for local testing
[System.Net.ServicePointManager]::ServerCertificateValidationCallback = { $true }

Write-Host "1. Testing Login..." -ForegroundColor Cyan
$loginBody = @{
    email = "superadmin@modulos.com"
    password = "Jlvm2612@"
} | ConvertTo-Json

try {
    $loginResponse = Invoke-RestMethod -Uri $loginUrl -Method Post -Body $loginBody -ContentType "application/json"
    $token = $loginResponse.token
    Write-Host "Login successful. Token obtained." -ForegroundColor Green
} catch {
    Write-Host "Login failed: $($_.Exception.Message)" -ForegroundColor Red
    exit
}

Write-Host "`n2. Testing Profile Update (PUT)..." -ForegroundColor Cyan
$profileBody = @{
    FirstName = "Super"
    LastName = "Admin Updated"
    DateOfBirth = "1990-01-01"
    Genero = 0
    PhoneNumber = "123456789"
    Address = "Calle Falsa 123"
} | ConvertTo-Json

$headers = @{
    Authorization = "Bearer $token"
}

try {
    $profileResponse = Invoke-RestMethod -Uri $profileUrl -Method Put -Body $profileBody -ContentType "application/json" -Headers $headers
    if ($profileResponse.success) {
        Write-Host "Profile update successful. New Last Name: $($profileResponse.user.lastName)" -ForegroundColor Green
    } else {
        Write-Host "Profile update reported failure: $($profileResponse.message)" -ForegroundColor Yellow
    }
} catch {
    Write-Host "Profile update failed: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host "`n3. Testing Change Password..." -ForegroundColor Cyan
$changePasswordBody = @{
    CurrentPassword = "Jlvm2612@"
    NewPassword = "Jlvm2612@Updated"
    ConfirmPassword = "Jlvm2612@Updated"
} | ConvertTo-Json

try {
    $changeResponse = Invoke-RestMethod -Uri $changePasswordUrl -Method Post -Body $changePasswordBody -ContentType "application/json" -Headers $headers
    if ($changeResponse.success) {
        Write-Host "Change password successful." -ForegroundColor Green
        
        # Revert password for future tests
        Write-Host "Reverting password..."
        $revertBody = @{
            CurrentPassword = "Jlvm2612@Updated"
            NewPassword = "Jlvm2612@"
            ConfirmPassword = "Jlvm2612@"
        } | ConvertTo-Json
        Invoke-RestMethod -Uri $changePasswordUrl -Method Post -Body $revertBody -ContentType "application/json" -Headers $headers
        Write-Host "Password reverted." -ForegroundColor Green
    } else {
        Write-Host "Change password reported failure: $($changeResponse.message)" -ForegroundColor Yellow
    }
} catch {
    Write-Host "Change password failed: $($_.Exception.Message)" -ForegroundColor Red
}
