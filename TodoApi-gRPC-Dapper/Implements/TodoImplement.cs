﻿using System;
using System.Threading.Tasks;
using Grpc;
using Grpc.Core;
using Grpc.Core.Utils;
using Proto.Todo;
using TodoApi_gRPC_Dapper.Models;
using TodoApi_gRPC_Dapper.Models.Persistance;

namespace TodoApi_gRPC_Dapper.Implements {
    public class TodoImplement : TodoService.TodoServiceBase {
        private IUnitOfWork _unitOfWork;

        public TodoImplement (IUnitOfWork unitOfWork) : base () {
            _unitOfWork = unitOfWork;
            if (_unitOfWork.TodoItems.Count () == 0) {
                _unitOfWork.TodoItems.Add (new Todo { Name = "Item1", IsComplete = false });
            }
        }

        public override async Task<GetTodoItemsResponse> GetTodoItems (Empty request, ServerCallContext context) {
            var result = await _unitOfWork.TodoItems.FindAllAsync ();
            var response = new GetTodoItemsResponse ();
            foreach (var todo in result) {
                response.Todos.Add (todo);
            }

            return response;
        }

        public override async Task<GetTodoItemResponse> GetTodoItem (GetTodoItemRequest request, ServerCallContext context) {
            var todo = await _unitOfWork.TodoItems.FindAsync (request.Id);
            var response = new GetTodoItemResponse { Todo = todo };

            return response;
        }

        public override async Task<PostTodoItemResponse> PostTodoItem (PostTodoItemRequest request, ServerCallContext context) {
            var newTodo = new Todo { Id = Guid.NewGuid().ToString(), Name = request.Name, IsComplete = false };
            await _unitOfWork.TodoItems.Add(newTodo);
            var response = new PostTodoItemResponse { Todo = newTodo };

            return response;
        }

        public override async Task<PutTodoItemResponse> PutTodoItem (PutTodoItemRequest request, ServerCallContext context) {
            await _unitOfWork.TodoItems.Update (request.Todo);
            var updatedTodoItem = await _unitOfWork.TodoItems.FindAsync (request.Todo.Id);
            var response = new PutTodoItemResponse { Todo = updatedTodoItem };

            return response;
        }

        public override async Task<DeleteTodoItemResponse> DeleteTodoItem (DeleteTodoItemRequest request, ServerCallContext context) {
            var deleteTodoItem = await _unitOfWork.TodoItems.FindAsync (request.Id);
            // deleteTodoItemがなかったときの例外処理必要
            await _unitOfWork.TodoItems.Remove (deleteTodoItem);
            var response = new DeleteTodoItemResponse { Todo = deleteTodoItem };

            return response;
        }
    }
}