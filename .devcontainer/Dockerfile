FROM mcr.microsoft.com/dotnet/sdk:8.0

ARG USERNAME=dotnet
ARG USER_UID=1000
ARG USER_GID=1000

RUN apt-get update && apt-get install -y \
    git \
    procps \
    curl \
    wget \
    build-essential \
    cmake \
    clang \
    mingw-w64 \
    iproute2 \
    unzip \
    zip \
    file \
    && rm -rf /var/lib/apt/lists/*

RUN groupadd --gid ${USER_GID} ${USERNAME} \
    && useradd --uid ${USER_UID} --gid ${USER_GID} \
       --create-home --shell /bin/bash ${USERNAME}

RUN echo "alias ls='ls --color=auto'" >> /home/${USERNAME}/.bashrc \
    && echo "alias ll='ls -alF'" >> /home/${USERNAME}/.bashrc \
    && echo "alias la='ls -A'" >> /home/${USERNAME}/.bashrc \
    && echo "alias l='ls -CF'" >> /home/${USERNAME}/.bashrc \
    && echo "alias ..='cd ..'" >> /home/${USERNAME}/.bashrc

ENV PATH="/home/${USERNAME}/.dotnet/tools:${PATH}"

WORKDIR /workspace

USER ${USERNAME}