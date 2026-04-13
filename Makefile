DOCKER_COMPOSE = docker-compose
  
GREEN = \033[0;32m
NC = \033[0m
  
.PHONY: dev
dev:
	@echo "${GREEN}Starting all services in development mode...${NC}"
	$(DOCKER_COMPOSE) up
  
.PHONY: down
down:
	@echo "${GREEN}Stopping all services...${NC}"
	$(DOCKER_COMPOSE) down