# Script to add products with images via Admin API
$apiUrl = "http://localhost:5299/api/admin/products"
$imagePath = "d:\DoAnMTKPM_WebBanNuoc\WebBanNuoc\Content\images\products"

$products = @(
    @{Name="Tra Sua Tran Chau Duong Den"; CategoryId=1; Price=45000; Image="product-1.png"; Description="Tra sua thom ngon voi tran chau duong den dai dai"},
    @{Name="Tra Sua O Long"; CategoryId=1; Price=40000; Image="product-2.png"; Description="Tra sua o long thom nong, vi tra dam da"},
    @{Name="Tra Sua Matcha"; CategoryId=1; Price=42000; Image="product-3.png"; Description="Tra sua matcha xanh mat, vi ngot diu"},
    @{Name="Tra Sua Khoai Mon"; CategoryId=1; Price=43000; Image="product-4.png"; Description="Tra sua khoai mon beo ngay thom ngon"},
    @{Name="Tra Sua Chocolate"; CategoryId=1; Price=45000; Image="product-5.png"; Description="Tra sua chocolate dam da, ngot ngao"},
    
    @{Name="Tra Dao Cam Sa"; CategoryId=2; Price=35000; Image="product-6.png"; Description="Tra dao cam sa tuoi mat, huong vi doc dao"},
    @{Name="Tra Vai"; CategoryId=2; Price=33000; Image="product-7.png"; Description="Tra vai ngot diu, huong thom quyen ru"},
    @{Name="Tra Chanh Leo"; CategoryId=2; Price=32000; Image="product-8.png"; Description="Tra chanh leo chua ngot, sang khoai"},
    @{Name="Tra Dau"; CategoryId=2; Price=34000; Image="product-9.png"; Description="Tra dau tay tuoi ngon, mau do dep mat"},
    @{Name="Tra Xoai"; CategoryId=2; Price=35000; Image="product-10.png"; Description="Tra xoai ngot thanh, huong vi nhiet doi"},
    
    @{Name="Ca Phe Den Da"; CategoryId=4; Price=25000; Image="product-11.png"; Description="Ca phe den da dam da, tinh tao"},
    @{Name="Ca Phe Sua"; CategoryId=4; Price=28000; Image="product-12.png"; Description="Ca phe sua ngot beo, truyen thong Viet"},
    @{Name="Bac Xiu"; CategoryId=4; Price=30000; Image="product-13.png"; Description="Bac xiu ngot diu, it dang"},
    @{Name="Cappuccino"; CategoryId=4; Price=38000; Image="product-14.png"; Description="Cappuccino Y thom nong, bot sua min"},
    @{Name="Latte"; CategoryId=4; Price=40000; Image="product-15.png"; Description="Latte mem mai, can bang sua va ca phe"},
    
    @{Name="Sinh To Bo"; CategoryId=5; Price=38000; Image="product-16.png"; Description="Sinh to bo beo ngay, bo duong"},
    @{Name="Sinh To Xoai"; CategoryId=5; Price=35000; Image="product-17.png"; Description="Sinh to xoai ngot mat, vitamin C cao"},
    @{Name="Sinh To Dau"; CategoryId=5; Price=36000; Image="product-18.png"; Description="Sinh to dau tuoi, mau do hong dep mat"},
    @{Name="Sinh To Sapoche"; CategoryId=5; Price=37000; Image="product-19.png"; Description="Sinh to sapoche thom ngot, giau dinh duong"},
    @{Name="Sinh To Dua Hau"; CategoryId=5; Price=33000; Image="product-20.png"; Description="Sinh to dua hau mat lanh, giai nhiet"},
    
    @{Name="Nuoc Ep Cam"; CategoryId=6; Price=30000; Image="product-21.png"; Description="Nuoc ep cam tuoi, giau vitamin C"},
    @{Name="Nuoc Ep Oi"; CategoryId=6; Price=28000; Image="product-22.png"; Description="Nuoc ep oi ngot thanh, tot cho suc khoe"},
    @{Name="Nuoc Ep Dua"; CategoryId=6; Price=32000; Image="product-23.png"; Description="Nuoc ep dua chua ngot, ho tro tieu hoa"},
    @{Name="Nuoc Ep Ca Rot"; CategoryId=6; Price=29000; Image="product-24.png"; Description="Nuoc ep ca rot tot cho mat, ngot tu nhien"},
    
    @{Name="Soda Viet Quat"; CategoryId=7; Price=35000; Image="product-25.png"; Description="Soda viet quat sang khoai, nhieu antioxidant"},
    @{Name="Soda Chanh"; CategoryId=7; Price=30000; Image="product-26.png"; Description="Soda chanh chua mat, giai khat tuyet voi"},
    @{Name="Soda Dau"; CategoryId=7; Price=33000; Image="product-27.png"; Description="Soda dau ngot diu, mau sac bat mat"},
    
    @{Name="Banh Croissant"; CategoryId=8; Price=25000; Image="product-28.png"; Description="Banh croissant bo, xop gion thom ngon"},
    @{Name="Banh Tiramisu"; CategoryId=8; Price=45000; Image="product-29.png"; Description="Banh tiramisu Y, kem pho mai mascarpone"},
    @{Name="Banh Red Velvet"; CategoryId=8; Price=48000; Image="product-30.png"; Description="Banh red velvet mem min, cream cheese"}
)

Write-Host "=========================================="-ForegroundColor Cyan
Write-Host "ADDING PRODUCTS WITH IMAGES" -ForegroundColor Cyan
Write-Host "Total products: $($products.Count)" -ForegroundColor Cyan
Write-Host "=========================================="-ForegroundColor Cyan
Write-Host ""

$successCount = 0
$failCount = 0

foreach ($product in $products) {
    $imageFile = Join-Path $imagePath $product.Image
    
    if (-not (Test-Path $imageFile)) {
        Write-Host "Image not found: $($product.Image)" -ForegroundColor Red
        $failCount++
        continue
    }
    
    try {
        $imageBytes = [System.IO.File]::ReadAllBytes($imageFile)
        $imageBase64 = [System.Convert]::ToBase64String($imageBytes)
        $extension = [System.IO.Path]::GetExtension($imageFile).ToLower()
        $mimeType = if ($extension -eq ".png") { "image/png" } else { "image/jpeg" }
        $imageDataUri = "data:$mimeType;base64,$imageBase64"
        
        $productData = @{
            name = $product.Name
            description = $product.Description
            basePrice = $product.Price
            categoryId = $product.CategoryId
            imageUrl = $imageDataUri
            isActive = $true
        } | ConvertTo-Json
        
        Write-Host "Adding: $($product.Name) (Size: $([math]::Round($imageBytes.Length / 1KB, 2)) KB)..." -NoNewline
        
        $response = Invoke-RestMethod -Uri $apiUrl -Method Post -Body $productData -ContentType "application/json" -ErrorAction Stop
        
        if ($response.success) {
            Write-Host " SUCCESS" -ForegroundColor Green
            $successCount++
        } else {
            Write-Host " FAILED: $($response.message)" -ForegroundColor Red
            $failCount++
        }
        
        Start-Sleep -Milliseconds 200
        
    } catch {
        Write-Host " ERROR: $($_.Exception.Message)" -ForegroundColor Red
        $failCount++
    }
}

Write-Host ""
Write-Host "=========================================="-ForegroundColor Cyan
Write-Host "SUMMARY" -ForegroundColor Cyan
Write-Host "Success: $successCount" -ForegroundColor Green
Write-Host "Failed: $failCount" -ForegroundColor Red
Write-Host "=========================================="-ForegroundColor Cyan
