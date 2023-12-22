#!/bin/bash
urlencode() {
  python -c 'import html,sys; print html.escape(sys.argv[1], sys.argv[2])' \
    "$1" "$urlencode_safe"
}

ExpiryTime=$(($(date +%s) + 7776000))
ProjectName="GGJ2024"

ScriptDirectory=$(dirname "$0")

Platform=$1
ArtifactPath=$2
Extension=$3

FileName=$ProjectName-$Platform-$(date +'%Y-%m')$Extension

rm "$FileName" || true
mv "$ArtifactPath" "$FileName"

ArtifactURL="https://builds.connectedplay.io/$ProjectName/$FileName"
InstallURL="https://builds.connectedplay.io/$ProjectName/install_$Platform.html"

PrivateKey=~/CloudFront/private_connectedplay_builds_key.pem

echo $FileName

Policy="{
    \"Statement\": [
        {
            \"Resource\": \"https://builds.connectedplay.io/$ProjectName/*\",
            \"Condition\": {
                \"DateLessThan\": {
                    \"AWS:EpochTime\": $ExpiryTime
                }
            }
        }
    ]
}"

Base64Policy=$(echo "$Policy" | tr -d "\n" | tr -d " \t\n\r" | openssl base64 | tr -- '+=/' '-_~' | tr -d "\n" | tr -d " \t\n\r")
SignedPolicy=$(echo "$Policy" | tr -d "\n" | tr -d " \t\n\r" | openssl sha1 -sign $PrivateKey | openssl base64 | tr -- '+=/' '-_~'| tr -d "\n" | tr -d " \t\n\r") 

SignedArtifactURL="$ArtifactURL?Policy=$Base64Policy&Signature=$SignedPolicy&Key-Pair-Id=$CloudFrontKeyPairID"
SignedInstallURL="$InstallURL?Policy=$Base64Policy&Signature=$SignedPolicy&Key-Pair-Id=$CloudFrontKeyPairID"

DownloadURL=$(printf '%s\n' "$SignedArtifactURL" | gsed -e 's@[\/&]@\\&@g')

FileSize=$(ls -lah $FileName | awk -F " " {'print $5'})

echo "Replacing {build_number}"
gsed -i 's@{build_number}@'$RevisionNumber'@g' "$ScriptDirectory/install.html"
echo "Replacing {project_name}"
gsed -i 's@{project_name}@'$ProjectName'@g' "$ScriptDirectory/install.html"
echo "Replacing {platform_name}"
gsed -i 's@{platform_name}@'$Platform'@g' "$ScriptDirectory/install.html"
echo "Replacing {build_time}"
gsed -i "s@{build_time}@$(date)@g" "$ScriptDirectory/install.html"
echo "Replacing {file_size}"
gsed -i 's@{file_size}@'$FileSize'@g' "$ScriptDirectory/install.html"
echo "Replacing {download_url}"
gsed -i 's@{download_url}@'$DownloadURL'@g' "$ScriptDirectory/install.html"

aws s3 cp "$FileName" s3://builds.connectedplay.io/$ProjectName/$FileName --profile $AWSProfile
aws s3 cp "$ScriptDirectory/install.html" s3://builds.connectedplay.io/$ProjectName/install_$Platform.html --profile $AWSProfile

aws cloudfront create-invalidation \
    --distribution-id $CloudFrontDistID \
    --paths "/$ProjectName/*" --profile $AWSProfile

GitVersionHash=$(git rev-parse --short HEAD)
DateNow=$(date '+%Y-%m-%d %H:%M:%S')
DiscordPostData="{\
    \"content\": \"New [$Platform build #$GitVersionHash ($FileSize)]($SignedInstallURL) uploaded at $DateNow\"
}"
curl -i -H "Accept: application/json" -H "Content-Type:application/json" \
-X POST --data "$DiscordPostData" "$DiscordWebhook"