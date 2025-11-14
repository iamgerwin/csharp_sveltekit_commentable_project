#!/bin/bash

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Project root directory
PROJECT_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
API_DIR="$PROJECT_ROOT/apps/api"
WEB_DIR="$PROJECT_ROOT/apps/web"

# Function to print colored messages
print_info() {
    echo -e "${BLUE}ℹ ${NC}$1"
}

print_success() {
    echo -e "${GREEN}✓ ${NC}$1"
}

print_error() {
    echo -e "${RED}✗ ${NC}$1"
}

print_warning() {
    echo -e "${YELLOW}⚠ ${NC}$1"
}

# Function to check if command exists
command_exists() {
    command -v "$1" >/dev/null 2>&1
}

# Function to check prerequisites
check_prerequisites() {
    print_info "Checking prerequisites..."

    local missing_deps=0

    if ! command_exists node; then
        print_error "Node.js is not installed"
        missing_deps=1
    else
        print_success "Node.js $(node --version)"
    fi

    if ! command_exists npm; then
        print_error "npm is not installed"
        missing_deps=1
    else
        print_success "npm $(npm --version)"
    fi

    if ! command_exists dotnet; then
        print_error ".NET SDK is not installed"
        missing_deps=1
    else
        print_success ".NET $(dotnet --version)"
    fi

    if [ $missing_deps -eq 1 ]; then
        print_error "Please install missing dependencies"
        exit 1
    fi

    echo ""
}

# Function to install dependencies
install_dependencies() {
    print_info "Installing dependencies..."

    # Install root dependencies
    print_info "Installing root dependencies..."
    npm install

    # Build shared packages
    print_info "Building shared packages..."
    npm run build --workspace=@commentable/shared-enums
    npm run build --workspace=@commentable/shared-types
    npm run build --workspace=@commentable/shared-constants

    # Install web dependencies
    if [ -f "$WEB_DIR/package.json" ]; then
        print_info "Installing web dependencies..."
        cd "$WEB_DIR" && npm install
        cd "$PROJECT_ROOT"
    fi

    # Restore API dependencies
    if [ -f "$API_DIR/API.csproj" ] || [ -f "$API_DIR/API.sln" ]; then
        print_info "Restoring API dependencies..."
        cd "$API_DIR" && dotnet restore
        cd "$PROJECT_ROOT"
    fi

    print_success "Dependencies installed successfully"
    echo ""
}

# Function to run database migrations
run_migrations() {
    print_info "Running database migrations..."

    if [ -f "$API_DIR/API.csproj" ] || [ -f "$API_DIR/API.sln" ]; then
        cd "$API_DIR"

        if dotnet ef database update; then
            print_success "Database migrations completed"
        else
            print_warning "Database migrations failed or not configured yet"
        fi

        cd "$PROJECT_ROOT"
    else
        print_warning "API project not found, skipping migrations"
    fi

    echo ""
}

# Function to run API
run_api() {
    print_info "Starting API server..."

    if [ -f "$API_DIR/API.csproj" ] || [ -f "$API_DIR/API.sln" ]; then
        cd "$API_DIR"
        dotnet run
    else
        print_error "API project not found at $API_DIR"
        exit 1
    fi
}

# Function to run Web
run_web() {
    print_info "Starting Web server..."

    if [ -f "$WEB_DIR/package.json" ]; then
        cd "$WEB_DIR"
        npm run dev
    else
        print_error "Web project not found at $WEB_DIR"
        exit 1
    fi
}

# Function to run both services
run_all() {
    print_info "Starting all services..."
    echo ""

    # Check if tmux is available for split screen
    if command_exists tmux; then
        print_info "Using tmux for split screen..."

        # Create new tmux session
        tmux new-session -d -s commentable "cd $API_DIR && dotnet run"
        tmux split-window -v -t commentable "cd $WEB_DIR && npm run dev"
        tmux select-layout -t commentable even-vertical
        tmux attach-session -t commentable
    else
        print_warning "tmux not found. Running services in sequence..."
        print_info "Install tmux for better experience: brew install tmux (macOS) or apt-get install tmux (Linux)"
        echo ""
        print_info "Starting API in background..."

        cd "$API_DIR"
        dotnet run &
        API_PID=$!

        sleep 5

        print_info "Starting Web..."
        cd "$WEB_DIR"
        npm run dev

        # Cleanup on exit
        trap "kill $API_PID" EXIT
    fi
}

# Function to build all projects
build_all() {
    print_info "Building all projects..."

    # Build shared packages
    print_info "Building shared packages..."
    npm run build

    # Build API
    if [ -f "$API_DIR/API.csproj" ] || [ -f "$API_DIR/API.sln" ]; then
        print_info "Building API..."
        cd "$API_DIR" && dotnet build
        cd "$PROJECT_ROOT"
    fi

    # Build Web
    if [ -f "$WEB_DIR/package.json" ]; then
        print_info "Building Web..."
        cd "$WEB_DIR" && npm run build
        cd "$PROJECT_ROOT"
    fi

    print_success "Build completed successfully"
    echo ""
}

# Function to clean all projects
clean_all() {
    print_info "Cleaning all projects..."

    # Clean node_modules and build artifacts
    print_info "Cleaning Node.js artifacts..."
    find . -name "node_modules" -type d -prune -exec rm -rf '{}' +
    find . -name "dist" -type d -prune -exec rm -rf '{}' +
    find . -name ".turbo" -type d -prune -exec rm -rf '{}' +
    find . -name ".svelte-kit" -type d -prune -exec rm -rf '{}' +

    # Clean .NET artifacts
    if [ -f "$API_DIR/API.csproj" ] || [ -f "$API_DIR/API.sln" ]; then
        print_info "Cleaning .NET artifacts..."
        cd "$API_DIR" && dotnet clean
        cd "$PROJECT_ROOT"
    fi

    print_success "Clean completed successfully"
    echo ""
}

# Function to run tests
run_tests() {
    print_info "Running tests..."

    # Run API tests
    if [ -f "$API_DIR/API.csproj" ] || [ -f "$API_DIR/API.sln" ]; then
        print_info "Running API tests..."
        cd "$API_DIR" && dotnet test
        cd "$PROJECT_ROOT"
    fi

    # Run Web tests
    if [ -f "$WEB_DIR/package.json" ]; then
        print_info "Running Web tests..."
        cd "$WEB_DIR" && npm test
        cd "$PROJECT_ROOT"
    fi

    print_success "Tests completed"
    echo ""
}

# Function to show usage
show_usage() {
    cat << EOF
${BLUE}MonoRepo Runner Script${NC}

${GREEN}Usage:${NC}
  ./run.sh [command]

${GREEN}Commands:${NC}
  check         Check prerequisites
  install       Install all dependencies
  migrate       Run database migrations
  api           Run API server only
  web           Run Web server only
  dev           Run all services (API + Web)
  build         Build all projects
  test          Run all tests
  clean         Clean all build artifacts
  help          Show this help message

${GREEN}Examples:${NC}
  ./run.sh check          # Check if all prerequisites are installed
  ./run.sh install        # Install all dependencies
  ./run.sh dev            # Start all services in development mode
  ./run.sh api            # Run only the API server
  ./run.sh web            # Run only the Web server
  ./run.sh build          # Build all projects
  ./run.sh clean          # Clean all build artifacts

${YELLOW}Note:${NC}
  - For better experience with 'dev' command, install tmux
  - Make sure to configure .env files before running
  - PostgreSQL and Redis should be running for full functionality

EOF
}

# Main script logic
main() {
    cd "$PROJECT_ROOT"

    case "${1:-}" in
        check)
            check_prerequisites
            ;;
        install)
            check_prerequisites
            install_dependencies
            ;;
        migrate)
            run_migrations
            ;;
        api)
            run_api
            ;;
        web)
            run_web
            ;;
        dev)
            check_prerequisites
            run_all
            ;;
        build)
            check_prerequisites
            build_all
            ;;
        test)
            check_prerequisites
            run_tests
            ;;
        clean)
            clean_all
            ;;
        help|--help|-h)
            show_usage
            ;;
        *)
            if [ -z "${1:-}" ]; then
                print_error "No command specified"
            else
                print_error "Unknown command: $1"
            fi
            echo ""
            show_usage
            exit 1
            ;;
    esac
}

# Run main function
main "$@"
