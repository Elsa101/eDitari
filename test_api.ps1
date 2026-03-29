$body = @{
    emailOrUsername = "admin1@editari.com"
    password = "Admin123!"
} | ConvertTo-Json

$loginResponse = Invoke-RestMethod -Uri "http://localhost:5102/api/Auth/login" -Method Post -Body $body -ContentType "application/json"
$token = $loginResponse.accessToken

Write-Host "Got token: $token"

# Get a student ID
$students = Invoke-RestMethod -Uri "http://localhost:5102/api/Students/with-parents" -Method Get -Headers @{ Authorization = "Bearer $token" }
if ($students.Count -gt 0) {
    $studentId = $students[0].studentId
    Write-Host "Deleting student ID: $studentId"
    
    try {
        Invoke-RestMethod -Uri "http://localhost:5102/api/Students/$studentId" -Method Delete -Headers @{ Authorization = "Bearer $token" }
        Write-Host "Deleted student successfully"
    } catch {
        Write-Host "ERROR deleting student:"
        $_.Exception.Response | Select-Object -ExpandProperty StatusCode
        $stream = $_.Exception.Response.GetResponseStream()
        $reader = New-Object System.IO.StreamReader($stream)
        $reader.ReadToEnd()
    }
} else {
    Write-Host "No students found."
}

# Get a staff/teacher ID
$staff = Invoke-RestMethod -Uri "http://localhost:5102/api/Staff" -Method Get -Headers @{ Authorization = "Bearer $token" }
if ($staff.Count -gt 0) {
    $staffId = $staff[0].staffId
    Write-Host "Deleting staff ID: $staffId"
    
    try {
        Invoke-RestMethod -Uri "http://localhost:5102/api/Staff/$staffId" -Method Delete -Headers @{ Authorization = "Bearer $token" }
        Write-Host "Deleted staff successfully"
    } catch {
        Write-Host "ERROR deleting staff:"
        $_.Exception.Response | Select-Object -ExpandProperty StatusCode
        $stream = $_.Exception.Response.GetResponseStream()
        $reader = New-Object System.IO.StreamReader($stream)
        $reader.ReadToEnd()
    }
} else {
    Write-Host "No staff found."
}
