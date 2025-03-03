$ENVIRONMENT_CODE = "int"
$gedApiAddress = "https://api-ged-intra.$ENVIRONMENT_CODE.maf.local/v2/"

function Get-RandomWord {
    $chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ"
    return -join (1..10 | ForEach-Object { $chars[(Get-Random -Minimum 0 -Maximum $chars.Length)] })
}

function Upload-File {
    param (
        [string]$filePath,
        [string]$fileName
    )
    
    $form = @{ file = Get-Item -Path $filePath }
    $response = Invoke-RestMethod -Uri "$gedApiAddress/upload" -Method Post -Form $form
    
    return $response.guidFile
}

function Finalize-Upload {
    param (
        [string]$documentUploadId,
        [string]$fileName,
        [hashtable]$documentMetadata
    )
    
    $documentMetadata["fileId"] = $documentUploadId
    $documentMetadata["fichierNom"] = $fileName
    
    $jsonContent = $documentMetadata | ConvertTo-Json -Depth 10
    
    $response = Invoke-RestMethod -Uri "$gedApiAddress/finalizeUpload" -Method Post -Body $jsonContent -ContentType "application/json"
    return $response.documentId
}

function Upload-DocumentToGed {
    param (
        [string]$filePath,
        [hashtable]$documentMetadata
    )
    
    $fileName = [System.IO.Path]::GetFileName($filePath)
    $documentUploadId = Upload-File -filePath $filePath -fileName $fileName
    $documentId = Finalize-Upload -documentUploadId $documentUploadId -fileName $fileName -documentMetadata $documentMetadata
    return $documentId
}

function Main {
    $filePath = "C:\Users\deschaseauxr\Documents\MAFlyDoc\test.pdf"
    $documentMetadata = @{ 
        deposePar = "ROD"
        dateDocument = (Get-Date).ToUniversalTime()
        categoriesFamille = "DOCUMENTS PERSONNES"
        categoriesCote = "IDENTITE"
        canalId = 1
    }
    
    $nbDocumentsToUpload = 6
    $uploadDocuments = @()
    
    for ($index = 0; $index -lt $nbDocumentsToUpload; $index++) {
        $metadataClone = $documentMetadata.Clone()
        $metadataClone["libelle"] = "{0}-{1}" -f ($index + 1), (Get-RandomWord)
        $metadataClone["categoriesTypeDocument"] = if ($index % 2 -eq 0) { "KBIS" } else { "PIECE IDENTITE" }
        
        $documentId = Upload-DocumentToGed -filePath $filePath -documentMetadata $metadataClone
        $uploadDocuments += $documentId
    }
    
    Write-Output $uploadDocuments
}

Main
