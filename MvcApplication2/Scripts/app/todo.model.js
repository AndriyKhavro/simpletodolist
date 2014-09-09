(function (ko, datacontext) {
    datacontext.todoItem = todoItem;
    datacontext.todoList = todoList;

    function todoItem(data) {
        var self = this;
        data = data || {};

        // Persisted properties
        self.todoItemId = data.todoItemId;
        self.title = ko.observable(data.title);
        self.isDone = ko.observable(data.isDone);
        self.todoListId = data.todoListId;
        self.deadline = ko.observable((new Date(data.deadline)).toJSON().substr(0, 10));
        self.priority = ko.observable(data.priority);

        // Non-persisted properties
        self.errorMessage = ko.observable();
        self.isEditingTitle = ko.observable(false);
        self.isHovered = ko.observable(false);

        saveChanges = function () {
            return datacontext.saveChangedTodoItem(self);
        };

        // Auto-save when these properties change
        self.isDone.subscribe(saveChanges);
        self.title.subscribe(saveChanges);
        self.deadline.subscribe(saveChanges);
        self.priority.subscribe(saveChanges);

        self.beginEdit = function () {
            self.isEditingTitle(true);
        }
        self.endEdit = function () {
            self.isEditingTitle(false);
        }

        self.showLinks = function () {
            self.isHovered(true);
        };
        self.hideLinks = function () {
            self.isHovered(false);
        };

        self.toJson = function () { return ko.toJSON(self) };
    };

    function todoList(data) {
        var self = this;
        data = data || {};

        // Persisted properties
        self.todoListId = data.todoListId;
        self.userId = data.userId || "to be replaced";
        self.title = ko.observable(data.title || "My todos");
        self.todos = ko.observableArray(importTodoItems(data.todos));

        // Non-persisted properties
        self.isEditingListTitle = ko.observable(false);
        self.newTodoTitle = ko.observable();
        self.errorMessage = ko.observable();
        self.isHovered = ko.observable(false);

        self.maxPriority = function () {
            return (self.todos().length === 0) ? -1 :
                Math.max.apply(Math, self.todos().map(function (o) { return o.priority(); }));
        };

        self.newDeadline = function() {
            var tomorrow = new Date();
            tomorrow.setDate(tomorrow.getDate() + 1);
            return tomorrow;
        };

        self.deleteTodo = function () {
            var todoItem = this;
            return datacontext.deleteTodoItem(todoItem)
                 .done(function () { self.todos.remove(todoItem); });
        };

        self.moveUpTodo = function () {
            var todoItem = this;
            var index = self.todos.indexOf(todoItem);
            if (index > 0) {
                var todoArray = self.todos();
                var priority = todoItem.priority();
                var itemToSwap = todoArray[index - 1];
                todoItem.priority(itemToSwap.priority());
                itemToSwap.priority(priority);
                self.todos.splice(index - 1, 2, todoItem, itemToSwap);
            }
        };

        self.moveDownTodo = function () {
            var todoItem = this;
            var index = self.todos.indexOf(todoItem);
            if (index < self.todos().length - 1 && index >= 0) {
                var todoArray = self.todos();
                var priority = todoItem.priority();
                var itemToSwap = todoArray[index + 1];
                todoItem.priority(itemToSwap.priority());
                itemToSwap.priority(priority);
                self.todos.splice(index, 2, itemToSwap, todoItem);
            }
        };

        self.beginEdit = function () {
            self.isEditingListTitle(true);
        }

        self.endEdit = function () {
            self.isEditingListTitle(false);
        }

        self.showLinks = function () {
            self.isHovered(true);
        };
        self.hideLinks = function () {
            self.isHovered(false);
        };

        // Auto-save when these properties change
        self.title.subscribe(function () {
            return datacontext.saveChangedTodoList(self);
        });

        self.toJson = function () { return ko.toJSON(self) };
    };
    // convert raw todoItem data objects into array of TodoItems
    function importTodoItems(todoItems) {
        /// <returns value="[new todoItem()]"></returns>
        return $.map(todoItems || [],
                function (todoItemData) {
                    return datacontext.createTodoItem(todoItemData);
                });
    }
    todoList.prototype.addTodo = function () {
        var self = this;
        if (self.newTodoTitle()) { // need a title to save
            var todoItem = datacontext.createTodoItem(
                {
                    title: self.newTodoTitle(),
                    todoListId: self.todoListId,
                    priority: self.maxPriority() + 1,
                    deadline: self.newDeadline()
                });
            self.todos.push(todoItem);
            datacontext.saveNewTodoItem(todoItem);
            self.newTodoTitle("");
        }
    };
    todoList.prototype.addTodoOnEnter = function (item, event) {
        var enterButtonKeyCode = 13;
        if (event.keyCode === enterButtonKeyCode) {
            this.addTodo();
        } else {
            return true;
        }
    };
})(ko, todoApp.datacontext);