#!/bin/bash

# Define colors for output formatting
GREEN='\033[0;32m'
CYAN='\033[0;36m'
BLUE='\033[0;34m'
PURPLE='\033[0;35m'
RED='\033[0;31m'
NC='\033[0m' # No Color

echo -e "${CYAN}==================================================${NC}"
echo -e "${CYAN}    HoangCN Multi-Project Dev Services Manager    ${NC}"
echo -e "${CYAN}==================================================${NC}"

# Array to keep track of background process IDs
PIDS=()

# Function to clean up background processes upon exit
cleanup() {
    echo -e "\n${RED}Stopping all development services...${NC}"
    for pid in "${PIDS[@]}"; do
        if kill -0 "$pid" 2>/dev/null; then
            # Kill the process and all its children (process group kill if possible)
            pkill -P "$pid" 2>/dev/null
            kill "$pid" 2>/dev/null
        fi
    done
    echo -e "${GREEN}All services stopped successfully.${NC}"
    exit 0
}

# Trap Ctrl+C (SIGINT) and exit signals to perform cleanup
trap cleanup SIGINT SIGTERM EXIT

# 1. Start HoangCN.MainSystem
echo -e "${BLUE}[1/4] Starting HoangCN.MainSystem (dotnet watch run)...${NC}"
(cd HoangCN.MainSystem && dotnet watch run) &
PIDS+=($!)

# 2. Start HoangCN.LearnMS
echo -e "${BLUE}[2/4] Starting HoangCN.LearnMS (dotnet watch run)...${NC}"
(cd HoangCN.LearnMS && dotnet watch run) &
PIDS+=($!)

# 3. Start Webs/CNAdmin
echo -e "${BLUE}[3/4] Starting Webs/CNAdmin (npm run dev)...${NC}"
(cd Webs/CNAdmin && npm run dev) &
PIDS+=($!)

# 4. Start Webs/CNLearnMS
echo -e "${BLUE}[4/4] Starting Webs/CNLearnMS (npm run dev)...${NC}"
(cd Webs/CNLearnMS && npm run dev) &
PIDS+=($!)

echo -e "${CYAN}--------------------------------------------------${NC}"
echo -e "${GREEN}All 4 services started successfully in background!${NC}"
echo -e "${GREEN}Press [Ctrl+C] to stop all services simultaneously.${NC}"
echo -e "${CYAN}--------------------------------------------------${NC}"

# Keep the shell open and wait for child processes
wait
