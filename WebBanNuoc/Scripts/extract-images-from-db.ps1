# Extract base64 images from database and save as files
$outputFolder = "d:\DoAnMTKPM_WebBanNuoc\WebBanNuoc\Content\images\products"

# Create folder if not exists
if (-not (Test-Path $outputFolder)) {
    New-Item -ItemType Directory -Path $outputFolder -Force | Out-Null
    Write-Host "Created folder: $outputFolder" -ForegroundColor Green
}

# Connect to database and extract images
$connectionString = "Server=DESKTOP-4EOK9DU\SQLEXPRESS02;Database=WebBanNuocDB;Integrated Security=true;"
$connection = New-Object System.Data.SqlClient.SqlConnection($connectionString)
$connection.Open()

$query = "SELECT Id, Name, ImageUrl FROM Products WHERE ImageUrl IS NOT NULL AND ImageUrl LIKE 'data:image%'"
$command = New-Object System.Data.SqlClient.SqlCommand($query, $connection)
$reader = $command.ExecuteReader()

$count = 0
$updates = @()

Write-Host "=========================================="-ForegroundColor Cyan
Write-Host "EXTRACTING IMAGES FROM DATABASE" -ForegroundColor Cyan
Write-Host "=========================================="-ForegroundColor Cyan

while ($reader.Read()) {
    $productId = $reader["Id"]
    $productName = $reader["Name"]
    $imageData = $reader["ImageUrl"]
    
    if ($imageData -match 'data:image/([^;]+);base64,(.+)') {
        $extension = $Matches[1]
        $base64 = $Matches[2]
        
        # Convert extension
        $ext = if ($extension -eq "jpeg") { "jpg" } else { $extension }
        
        # Create filename
        $filename = "product-$productId.$ext"
        $filepath = Join-Path $outputFolder $filename
        
        try {
            # Decode and save image
            $imageBytes = [System.Convert]::FromBase64String($base64)
            [System.IO.File]::WriteAllBytes($filepath, $imageBytes)
            
            $sizeKB = [math]::Round($imageBytes.Length / 1KB, 2)
            Write-Host "Extracted: $productName -> $filename ($sizeKB KB)" -ForegroundColor Green
            
            # Store update info
            $newPath = "/Content/images/products/$filename"
            $updates += @{Id=$productId; Path=$newPath}
            
            $count++
        }
        catch {
            Write-Host "ERROR extracting $productName : $($_.Exception.Message)" -ForegroundColor Red
        }
    }
}

$reader.Close()

# Update database with new paths
Write-Host "`n=========================================="-ForegroundColor Cyan
Write-Host "UPDATING DATABASE" -ForegroundColor Cyan
Write-Host "=========================================="-ForegroundColor Cyan

foreach ($update in $updates) {
    $updateQuery = "UPDATE Products SET ImageUrl = @Path WHERE Id = @Id"
    $updateCommand = New-Object System.Data.SqlClient.SqlCommand($updateQuery, $connection)
    $updateCommand.Parameters.AddWithValue("@Path", $update.Path) | Out-Null
    $updateCommand.Parameters.AddWithValue("@Id", $update.Id) | Out-Null
    $updateCommand.ExecuteNonQuery() | Out-Null
    Write-Host "Updated Product ID $($update.Id) -> $($update.Path)" -ForegroundColor Green
}

$connection.Close()

Write-Host "`n=========================================="-ForegroundColor Cyan
Write-Host "COMPLETED" -ForegroundColor Cyan
Write-Host "Extracted: $count images" -ForegroundColor Green
Write-Host "Saved to: $outputFolder" -ForegroundColor Green
Write-Host "=========================================="-ForegroundColor Cyan
