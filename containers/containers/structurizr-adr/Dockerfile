# Start from Structurizr Lite base image
FROM structurizr/lite:latest

# Install curl/unzip, download release zip, unpack and move to /opt, then clean up
RUN apt-get update && apt-get install -y curl unzip && \
    curl -sSL -o /tmp/adr-tools-3.0.0.zip https://github.com/npryce/adr-tools/archive/refs/tags/3.0.0.zip && \
    unzip /tmp/adr-tools-3.0.0.zip -d /tmp && \
    mv /tmp/adr-tools-3.0.0 /opt/adr-tools && \
    rm /tmp/adr-tools-3.0.0.zip && \
    apt-get remove -y unzip curl && apt-get autoremove -y && rm -rf /var/lib/apt/lists/*

# Add adr-tools to PATH
ENV PATH="/opt/adr-tools/src:${PATH}"

# Set working directory
WORKDIR /usr/local/structurizr
