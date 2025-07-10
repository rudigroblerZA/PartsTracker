provider "aws" {
  region = "af-south-1"
}

resource "aws_vpc" "main" {
  cidr_block = "10.0.0.0/16"
}

resource "aws_subnet" "public" {
  vpc_id     = aws_vpc.main.id
  cidr_block = "10.0.1.0/24"
}

resource "aws_db_subnet_group" "rds_subnet_group" {
  name       = "rds-subnets"
  subnet_ids = [aws_subnet.public.id]
}

resource "aws_db_instance" "postgres" {
  allocated_storage    = 20
  engine               = "postgres"
  engine_version       = "15"
  instance_class       = "db.t3.micro"
  username             = "partsuser"
  password             = "supersecret"
  skip_final_snapshot  = true
  publicly_accessible  = true
  db_subnet_group_name = aws_db_subnet_group.rds_subnet_group.name
}

resource "aws_ecs_cluster" "api_cluster" {
  name = "parts-tracker-cluster"
}

resource "aws_ecs_task_definition" "api_task" {
  family                   = "parts-tracker-task"
  requires_compatibilities = ["FARGATE"]
  cpu                      = "256"
  memory                   = "512"
  network_mode             = "awsvpc"

  container_definitions = jsonencode([
    {
      name  = "partstracker.ui"
      image = "rudigrobler/partstracker.ui:latest"
      portMappings = [
        {
          containerPort = 8081
        }
      ]
    },
    {
      name  = "partstracker.webapi"
      image = "rudigrobler/partstracker.webapi:latest"
      portMappings = [
        {
          containerPort = 8080
        }
      ],
      environment = [
        {
          name  = "ConnectionStrings__DefaultConnection"
          value = "Host=${aws_db_instance.postgres.address};Database=InventoryDb;Username=partsadmin;Password=supersecret"
        }
      ]
    }
  ])
}

resource "aws_ecs_service" "api_service" {
  name            = "parts-api-service"
  cluster         = aws_ecs_cluster.api_cluster.id
  task_definition = aws_ecs_task_definition.api_task.arn
  desired_count   = 1
  launch_type     = "FARGATE"
  network_configuration {
    subnets          = [aws_subnet.public.id]
    assign_public_ip = true
  }
}