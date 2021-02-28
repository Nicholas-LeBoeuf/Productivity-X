create table user_tbl (
	user_id integer unsigned auto_increment unique not null,
    firstName varchar(64),
    lastName varchar(64),
    username varchar(64),
    email varchar(60),
    passwordHash varchar(24),
    verificationCode integer unsigned null,
    PRIMARY KEY (user_id)
);

create table events_tbl (
	user_id integer unsigned not null,
    eventName varchar(25) unique not null,
    color varchar(25),
    event_date date not null,
    start_at time null,
    end_at time null,
    notification boolean not null,
    location varchar(20) not null,
	description varchar (100) null,
    guest_id integer unsigned not null,
	PRIMARY KEY (guest_id),
    FOREIGN KEY (user_id) REFERENCES user_tbl(user_id)
);

create table guest_tbl(
	user_id integer unsigned not null,
    guest_id integer unsigned not null,
    addGuest boolean not null,
    guest_email varchar(60) null,
	FOREIGN KEY (user_id) REFERENCES user_tbl(user_id),
    FOREIGN KEY (guest_id) REFERENCES events_tbl(guest_id)
);

create table category_tbl (
	user_id integer unsigned not null,
    category varchar(25) unique not null,
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
    email_friend varchar(60) not null,
    PRIMARY KEY (friend_id),
	FOREIGN KEY (user_id) REFERENCES user_tbl(user_id)
);

create table quickMessages_tbl(
	friend_id integer unsigned not null,
    message varchar (16) not null,
	FOREIGN KEY (friend_id) REFERENCES friends_tbl(friend_id)
);
