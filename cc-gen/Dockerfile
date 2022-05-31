# syntax=docker/dockerfile:experimental
FROM registry.gitlab.com/jitesoft/dockerfiles/alpine:latest
ARG VERSION
ARG BUILD_TIME

LABEL maintainer="Johannes Tegnér <johannes@jitesoft.com>" \
      maintainer.org="Jitesoft" \
      maintainer.org.uri="https://jitesoft.com" \
      com.jitesoft.project.repo.type="git" \
      com.jitesoft.project.repo.uri="https://gitlab.com/jitesoft/open-source/c-sharp/cc-gen" \
      com.jitesoft.project.repo.issues="https://gitlab.com/jitesoft/open-source/c-sharp/cc-gen/issues" \
      com.jitesoft.project.registry.uri="registry.gitlab.com/jitesoft/open-source/c-sharp/cc-gen" \
      com.jitesoft.app.cc-gen.version="${VERSION}" \
      # Open container labels
      org.opencontainers.image.version="${VERSION}" \
      org.opencontainers.image.created="${BUILD_TIME}" \
      org.opencontainers.image.description="cc-gen on alpine linux" \
      org.opencontainers.image.vendor="Jitesoft" \
      org.opencontainers.image.source="https://gitlab.com/jitesoft/open-source/c-sharp/cc-gen" \
      # Artifact hub annotations
      io.artifacthub.package.alternative-locations="oci://ghcr.io/jitesoft/cc-gen-sharp" \
      io.artifacthub.package.readme-url="https://gitlab.com/jitesoft/open-source/c-sharp/cc-gen/-/raw/master/README.md" \
      io.artifacthub.package.logo-url="https://jitesoft.com/favicon-96x96.png"

ARG TARGETARCH
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1

RUN --mount=type=bind,source=./release,target=/tmp \
    cp /tmp/${TARGETARCH}/cc-gen /usr/local/bin/ \
 && cp /tmp/${TARGETARCH}/libgit*.so /usr/local/lib/  \
 && apk add --no-cache libstdc++ libgcc \
 && addgroup -g 1000 cc-gen \
 && adduser -u 1000 -G cc-gen -s /bin/ash -D cc-gen

USER cc-gen
