create table user_tbl (
	user_id integer unsigned auto_increment unique not null,
    firstName varchar(64),
    lastName varchar(64),
	username varchar(64) not null,
    email varchar(60),
    password varchar(64),
    confirmpassword varchar(64),
    verificationCode varchar(16),
    PRIMARY KEY (user_id)
);

create table events_tbl (
	user_id integer unsigned not null,
	event_id integer unsigned auto_increment unique not null,
    eventName varchar(25) unique not null,
    event_date date not null,
    start_at time null,
    end_at time null,
    notification boolean not null,
    reminder int,
    location varchar(20) not null,
	description varchar (100) null,
	categoryname varchar(25),
	guest boolean not null,
	friend boolean not null,
	PRIMARY KEY (event_id),
    FOREIGN KEY (user_id) REFERENCES user_tbl(user_id)
);

create table guest_tbl(
	user_id integer unsigned not null,
	event_id integer unsigned not null,
    guest_id integer unsigned auto_increment unique not null,
	guest_username varchar(64) not null,
    guest_email varchar(60) null,
	isfriend boolean not null,
	FOREIGN KEY (user_id) REFERENCES user_tbl(user_id),
    FOREIGN KEY (event_id) REFERENCES events_tbl(event_id)
);

create table category_tbl (
	user_id integer unsigned not null,
    category_id integer unsigned auto_increment unique not null,
    categoryname varchar(25) unique not null,
    color varchar(25) not null,
    description varchar (100) null,
	FOREIGN KEY (user_id) REFERENCES user_tbl(user_id)
);

create table todo_tbl(
	user_id integer unsigned not null,
    taskName varchar(25) unique not null,
    finished boolean not null,
	FOREIGN KEY (user_id) REFERENCES user_tbl(user_id)
);

create table friends_tbl(
	user_id integer unsigned not null,
    friend_id integer unsigned unique not null,
	friend_username varchar(64) not null,
    friend_email varchar(60) null,
    PRIMARY KEY (friend_id),
	FOREIGN KEY (user_id) REFERENCES user_tbl(user_id)
);

create table quickMessages_tbl(
	friend_id integer unsigned not null,
    message varchar (16) not null,
	FOREIGN KEY (friend_id) REFERENCES friends_tbl(friend_id)
);
